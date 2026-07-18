namespace QuartzScheduler.API.Exceptions;

public abstract class AppException(string message) : Exception(message);

// the requested resource doesn't exist, 404
public class NotFoundException(string message) : AppException(message);

// the request conflicts with existing state, 409
public class ConflictException(string message) : AppException(message);

// the request is malformed or breaks a business rule, 400
public class BadRequestException(string message) : AppException(message);

// authenticated but not permitted, 403
public class ForbiddenException(string message) : AppException(message);